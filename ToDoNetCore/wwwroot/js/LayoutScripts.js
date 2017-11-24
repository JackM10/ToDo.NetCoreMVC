window.onload = function () {

	//Day-Night mode:
    var changeDayNightMode = document.getElementById('changeDayNightModeBtn');
    var isDayModeSelected = true;

    changeDayNightMode.onclick = function() {
        if (isDayModeSelected) {
            document.body.style.backgroundColor = "grey";
            isDayModeSelected = false;
        } else {
            document.body.style.backgroundColor = "white";
            isDayModeSelected = true;
        }
	};

	//request tooltip:
	changeDayNightMode.onmousemove = mouseMoveUnderChangeDayModeBtnHandler;
    changeDayNightMode.onmouseout = mouseOutFromChangeDayModeBtnHandler;

};

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

function mouseMoveUnderChangeDayModeBtnHandler(browserParameters) {
    if (!browserParameters) {
		browserParameters = window.event;
        tooltipInstance.show("Press to change day and night modes", browserParameters.clientX + 10, browserParameters.clientY + 10);
    }
}

function mouseOutFromChangeDayModeBtnHandler() {
    tooltipInstance.hide();
}