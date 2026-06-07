// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener('DOMContentLoaded', function () {
	var toggle = document.getElementById('themeToggle');
	if (!toggle) {
		return;
	}

	var root = document.documentElement;
	var label = toggle.querySelector('.theme-switch-label');
	var isButton = toggle.tagName === 'BUTTON';

	var applyTheme = function (theme) {
		if (theme === 'light') {
			root.setAttribute('data-theme', 'light');
			if (isButton) {
				toggle.setAttribute('aria-pressed', 'true');
			}
			localStorage.setItem('pc-theme', 'light');
			if (label) {
				label.textContent = 'Switch to Dark mode';
			}
			toggle.setAttribute('aria-label', 'Switch to dark mode');
		} else {
			root.removeAttribute('data-theme');
			if (isButton) {
				toggle.setAttribute('aria-pressed', 'false');
			}
			localStorage.setItem('pc-theme', 'dark');
			if (label) {
				label.textContent = 'Switch to Light mode';
			}
			toggle.setAttribute('aria-label', 'Switch to light mode');
		}
	};

	applyTheme(root.getAttribute('data-theme') === 'light' ? 'light' : localStorage.getItem('pc-theme'));

	toggle.addEventListener('click', function () {
		applyTheme(root.getAttribute('data-theme') === 'light' ? 'dark' : 'light');
	});
});
