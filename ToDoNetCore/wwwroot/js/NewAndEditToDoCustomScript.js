window.onbeforeunload = function () {
	var nameField = document.getElementById("nameField");
	var descrField = document.getElementById("descriptionField");

	if (nameField.value.length === 0 || descrField.value.length === 0) {
		return "Unsaved ToDo will be lost?";
	}
}

window.onload = function() {
	function onMouseOver(placeholder) {
		$(placeholder).css("background", "#e6f2ff");
	}

	function onMouseLeave(placeholder) {
		$(placeholder).css("background", "");
	}

	var a = $(".mouseOverAndLeave");

	$(".mouseOverAndLeave").mouseover(function () { onMouseOver(this); })
	$(".mouseOverAndLeave").mouseleave(function () { onMouseLeave(this); })
}