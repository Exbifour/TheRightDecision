class ProgressBar {
    constructor(id, numOfSteps)
    {
        this.numberOfSteps = numOfSteps;
        this.stepWidth = 100 / this.numberOfSteps;
        this.currentStep = 0;
        this.id = '#' + id;
        this.updateText();

        console.log(this.numberOfSteps + ' | ' + this.stepWidth);
    }

    Move() {
        this.currentStep++;
        $(this.id).css('width', this.currentStep * this.stepWidth + '%');
        if (this.currentStep == this.numberOfSteps) {
            $(this.id).removeClass('progress-bar-warning');
            $(this.id).addClass('progress-bar-success');
        }
        else if (this.currentStep > this.numberOfSteps * 0.35) {
            $(this.id).removeClass('progress-bar-danger');
            $(this.id).addClass('progress-bar-warning');
        }
        this.updateText();
    }

    Reset() {
        this.currentStep = 0;
        $(this.id).css('width', 0);
        $(this.id).text(this.currentStep);
        $(this.id).removeClass('progress-bar-warning, progress-bar-success');
        $(this.id).addClass('progress-bar-danger');
    }

    updateText() {
        $(this.id).text(this.currentStep + ' / ' + this.numberOfSteps);
    }
}

