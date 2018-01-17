//import React from 'react';
//import ReactDOM from 'react-dom';


$(window).load(function () {
	var button = $('#ajaxBtn');
	button.on("click",
		function () {
			//alert("tst");
			$.ajax('additionalDetails.html',
				{
					success: function(response) {
						button.html(response).slideDown();
					},
					error: function(request, errorType, errorMessage) {
						document.write(request, errorType, errorMessage);
					},
					timeout: 5000,
					beforeSend: function () {
						button.removeClass('btn');
						button.removeClass('btn-default');
						button.addClass('is-loading');
					},
					complete: function() {
						//button.removeClass('is-loading');
					}
				});
		});
});
