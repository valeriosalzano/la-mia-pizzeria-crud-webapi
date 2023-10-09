// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*---- Preloader Start -----*/
const preloader = $('#preloader');
if (preloader) {
    window.onload = function () {
    
        $('#preloader').className = "d-none";
        $('#preloader').delay(2000).fadeOut(500);
    };

}
/*---- Preloader End -----*/