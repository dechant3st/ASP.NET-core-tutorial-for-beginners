// for navbar
$(document).ready(function() {

    // Check for click events on the navbar burger icon
    $(".navbar-burger").click(function() {

        // Toggle the "is-active" class on both the "navbar-burger" and the "navbar-menu"
        $(".navbar-burger").toggleClass("is-active");
        $(".navbar-menu").toggleClass("is-active");

    });

    $(".input-validation-error").each(function (index) {
        this.classList.add("is-danger");
        $(this.parentNode).addClass("has-icons-right");
        $(this.parentNode).append('<span class="icon is-small is-right has-text-danger"><i class="fa fa-times"></i></span>');
    });

    $("input[type='file']").change(function () {
        $(this.parentNode).find(".file-name")[0].innerText = $(this).val().split('\\').pop();
    });
});
