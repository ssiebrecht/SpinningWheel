// Spotify IFrame API wrapper.
// Loads the official embed script on demand and exposes a thin controller
// surface that Blazor can invoke via IJSObjectReference.
//
// Reference: https://developer.spotify.com/documentation/embeds/references/iframe-api

let apiReadyPromise = null;

function loadApi() {
    if (apiReadyPromise) {
        return apiReadyPromise;
    }

    apiReadyPromise = new Promise((resolve) => {
        if (window.IFrameAPI) {
            resolve(window.IFrameAPI);
            return;
        }

        const previous = window.onSpotifyIframeApiReady;
        window.onSpotifyIframeApiReady = (IFrameAPI) => {
            window.IFrameAPI = IFrameAPI;
            if (typeof previous === 'function') {
                try { previous(IFrameAPI); } catch (_) { /* ignored */ }
            }
            resolve(IFrameAPI);
        };

        const existing = document.querySelector('script[data-spotify-iframe-api]');
        if (!existing) {
            const script = document.createElement('script');
            script.src = 'https://open.spotify.com/embed/iframe-api/v1';
            script.async = true;
            script.dataset.spotifyIframeApi = 'true';
            document.head.appendChild(script);
        }
    });

    return apiReadyPromise;
}

export async function createController(hostElement, uri, dotNetRef) {
    if (!hostElement) {
        return null;
    }

    const IFrameAPI = await loadApi();

    // Create an inner container so the Spotify API never touches the
    // Blazor-managed host element. The API may replace or detach its
    // target node, which would break Blazor's DOM diffing (removeChild
    // on a detached node has null parentNode).
    const container = document.createElement('div');
    hostElement.appendChild(container);

    return new Promise((resolve) => {
        const options = {
            uri: uri,
            width: '100%',
            height: '152',
        };

        IFrameAPI.createController(container, options, (controller) => {
            const handle = {
                controller,
                dotNetRef,
                hostElement,
            };

            controller.addListener('playback_update', (e) => {
                if (!dotNetRef) return;
                try {
                    dotNetRef.invokeMethodAsync(
                        'OnPlaybackUpdate',
                        !!(e.data && e.data.isPaused),
                        (e.data && typeof e.data.position === 'number') ? e.data.position : 0,
                    );
                } catch (_) { /* disposed */ }
            });

            resolve(handle);
        });
    });
}

export function loadUri(handle, uri) {
    if (!handle || !handle.controller) return;
    try {
        handle.controller.loadUri(uri);
    } catch (_) { /* controller may be destroyed */ }
}

export function play(handle) {
    if (!handle || !handle.controller) return;
    // `resume` falls back to `play` when no track is loaded yet; both are safe.
    try {
        if (typeof handle.controller.resume === 'function') {
            handle.controller.resume();
        } else {
            handle.controller.play();
        }
    } catch (_) { /* controller may be destroyed */ }
}

export function pause(handle) {
    if (!handle || !handle.controller) return;
    try {
        handle.controller.pause();
    } catch (_) { /* controller may be destroyed */ }
}

export function destroy(handle) {
    if (!handle) return;
    if (handle.controller) {
        try {
            handle.controller.destroy();
        } catch (_) { /* ignored - iframe may already be gone */ }
        handle.controller = null;
    }
    // Remove any leftover iframe / inner containers from the host element
    // so that Blazor finds a clean DOM when it removes the component.
    if (handle.hostElement) {
        try {
            handle.hostElement.textContent = '';
        } catch (_) { /* host may already be detached */ }
        handle.hostElement = null;
    }
    handle.dotNetRef = null;
}
