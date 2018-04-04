$(document).ready(function () {
	var toDoSuccessCreatedAlert = $("#toDoSuccessCreatedAlert");
	if (toDoSuccessCreatedAlert) {
		toDoSuccessCreatedAlert.fadeTo(2000, 500).slideUp(500,
			function() {
				toDoSuccessCreatedAlert.slideUp(500);
			});
	};
	$('tr').sortable();
})

