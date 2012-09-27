function EditProfileModel(data) {
    var self = this;
    self.Success = data.Success;
    self.Message = data.Message;
    self.Errors = data.Errors;
    self.ChangeEmail = data.ChangeEmail;
    self.Email = data.Email;
}