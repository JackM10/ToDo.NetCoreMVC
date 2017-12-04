window.onbeforeunload = function () {
	var nameField = document.getElementById("nameField");
	var descrField = document.getElementById("descriptionField");

    if (nameField.value.length != 0 || descrField.value.length != 0) {
        return "Unsaved ToDo will be lost?";
    }
}