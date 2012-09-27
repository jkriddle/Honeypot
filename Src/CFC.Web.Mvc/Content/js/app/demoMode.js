function DemoMode() {
    var self = this;
    self.DemoModeOn = false;
    self.DemoTimer;
}

DemoMode.prototype.Start = function () {
    var self = this;
    self.DemoModeOn = true;
    self.DemoTimer = setInterval(function () { self.CallDemo(); }, 5000);

};

DemoMode.prototype.Stop = function () {
    self.DemoModeOn = false;
    clearInterval(self.DemoTimer);
};

DemoMode.prototype.CallDemo = function () {
    $.ajax({
        url: CFC.Url('/Demo/Run'),
        dataType: 'json',
        traditional: true,
        cache: false
    });
};

    