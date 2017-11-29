window.onload = function () {

	//Day-Night mode control:
    var changeDayNightMode = $('changeDayNightModeBtn');
	var isDayModeSelected;

	if (window.localStorage.isDayModeSelected) {
		isDayModeSelected = window.localStorage.isDayModeSelected;
	}

    if (isDayModeSelected) {
        document.body.style.backgroundColor = "grey";
        isDayModeSelected = false;
    } else {
        document.body.style.backgroundColor = "white";
        isDayModeSelected = true;
    }

    changeDayNightMode.onclick = function() {
        if (isDayModeSelected) {
            document.body.style.backgroundColor = "grey";
			isDayModeSelected = false;
            //Saving day\night mode in local storage:
            window.localStorage.isDayModeSelected = "false";
        } else {
            document.body.style.backgroundColor = "white";
			isDayModeSelected = true;
            window.localStorage.isDayModeSelected = "true";
        }
	};

	//request tooltip:
	changeDayNightMode.onmousemove = mouseMoveUnderChangeDayModeBtnHandler;
    changeDayNightMode.onmouseout = mouseOutFromChangeDayModeBtnHandler;
};

function $(elementId) {
    return document.getElementById(elementId);
}

//ToolTip for Day-Night control:
function Tooltip() {
    this.tooltip = document.createElement("div");
    this.tooltip.style.position = "absolute";
    this.tooltip.style.visibility = "hidden";
    this.tooltip.className = "tooltip";
}

Tooltip.prototype.show = function (text, x, y) {
    this.tooltip.innerHTML = text;
    this.tooltip.style.left = x + "px";
    this.tooltip.style.top = y + "px";
    this.tooltip.style.visibility = "visible";

    if (this.tooltip.parentNode != document.body) {
        document.body.appendChild(this.tooltip);
    }
};

Tooltip.prototype.hide = function() {
    this.tooltip.style.visibility = "hidden";
};

var tooltipInstance = new Tooltip();

function mouseMoveUnderChangeDayModeBtnHandler(e) {
    if (!e) e = window.event;
        tooltipInstance.show("Press to change day and night modes", e.clientX + 10, e.clientY + 10);
}

function mouseOutFromChangeDayModeBtnHandler() {
    tooltipInstance.hide();
}

function findCookieValue(cookieName) {
	var allCookies = document.cookie;
	var pos = allCookies.indexOf(cookieName + "=");

	//If cookie exists, extract it's value:
    if (pos != -1) {
		var start = pos + cookieName.length + 1;
		var end = allCookies.indexOf(";", start);

        if (end == -1) {
            end = allCookies.length;
		}

		var value = allCookies.substring(start, end);

        return value;
    }
}