window.dropdownHelper = {
    toggleDropdown: function () {
        const dropdown = document.getElementById("myDropdown");
        if (dropdown) {
            dropdown.classList.toggle("show");
        }
    },
    closeDropdowns: function (e) {
        if (!e.target.matches('.dropbtn')) {
            var dropdowns = document.getElementsByClassName("dropdown-content");
            for (let i = 0; i < dropdowns.length; i++) {
                dropdowns[i].classList.remove('show');
            }
        }
    },
    registerOutsideClick: function () {
        window.addEventListener("click", window.dropdownHelper.closeDropdowns);
    }
};