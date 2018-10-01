window.onload = function () {
	//Day-Night mode control:
    var changeDayNightMode = GetDocumentElementByID('changeDayNightModeBtn');
	var isDayModeSelected;

	if (window.localStorage.isDayModeSelected) {
		isDayModeSelected = window.localStorage.isDayModeSelected;
	}

    if (isDayModeSelected) {
		document.body.style.background = "linear-gradient(90deg, #75a3a3, #3d5c5c)";
        isDayModeSelected = false;
    } else {
        document.body.style.backgroundColor = "white";
        isDayModeSelected = true;
    }

    changeDayNightMode.onclick = () => {
        if (isDayModeSelected) {
	        document.body.style.background = "linear-gradient(90deg, #75a3a3, #3d5c5c)";
	        isDayModeSelected = false;
            //Saving day\night mode in local storage:
            window.localStorage.isDayModeSelected = "false";
		} else {
			document.body.removeAttribute("style");
            document.body.style.backgroundColor = "white";
			isDayModeSelected = true;
            window.localStorage.isDayModeSelected = "true";
        }
	};
    
	//request tooltip:
	changeDayNightMode.addEventListener("mousemove", mouseMoveUnderChangeDayModeBtnHandler, false);
	changeDayNightMode.addEventListener("mouseleave", mouseOutFromChangeDayModeBtnHandler, false);
};

function GetDocumentElementByID(elementId) {
    return document.getElementById(elementId);
}

//ToolTip for Day-Night control:
function Tooltip() {
    this.tooltip = document.createElement("div");
    this.tooltip.style.position = "absolute";
    this.tooltip.style.visibility = "hidden";
    this.tooltip.className = "todo-tooltip";
}

Tooltip.prototype.show = function (text, x, y) {
    this.tooltip.innerHTML = text;
    this.tooltip.style.left = x + "px";
    this.tooltip.style.top = y + "px";
    this.tooltip.style.visibility = "visible";

    if (this.tooltip.parentNode !== document.body) {
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
