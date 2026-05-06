// Wheel DOM interop module.
// Provides helpers that Blazor cannot achieve through Razor alone:
// reading the current mid-transition rotation and snapping the rotor
// to a known position before starting a new CSS transition.

/**
 * Reads the current computed rotation of an SVG <g> element
 * by decomposing its CSS transform matrix.
 * @param {Element} element – The rotor <g> element.
 * @returns {number} Current rotation in degrees (0–360).
 */
export function getCurrentRotation(element) {
    if (!element) {
        return 0;
    }

    const style = getComputedStyle(element);
    const matrix = style.transform || style.webkitTransform;

    if (!matrix || matrix === 'none') {
        return 0;
    }

    // matrix(a, b, c, d, tx, ty)
    const values = matrix.match(/matrix\((.+)\)/);
    if (!values) {
        return 0;
    }

    const parts = values[1].split(',').map(Number);
    const a = parts[0];
    const b = parts[1];

    // atan2(b, a) gives the rotation in radians
    let degrees = Math.atan2(b, a) * (180 / Math.PI);
    if (degrees < 0) {
        degrees += 360;
    }
    return degrees;
}

/**
 * Instantly snaps the rotor element to the given rotation
 * by disabling the CSS transition and forcing a layout reflow.
 * This ensures the browser registers the new position before
 * the next Blazor render applies a deceleration transition.
 * @param {Element} element – The rotor <g> element.
 * @param {number} degrees – Absolute rotation in degrees.
 */
export function snapToRotation(element, degrees) {
    if (!element) {
        return;
    }

    element.style.transition = 'none';
    element.style.transform = `rotate(${degrees}deg)`;

    // Force reflow so the browser commits the snap position.
    element.getBoundingClientRect();
}
