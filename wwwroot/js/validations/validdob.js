// Wait for DOM + jQuery validator to be ready
$(function () {
    // Custom validation method
    $.validator.addMethod("validdob", function (value, element) {
        if (!value) return false;

        var dob = new Date(value);
        var today = new Date();
        var hundredYearsAgo = new Date();
        hundredYearsAgo.setFullYear(today.getFullYear() - 100);

        return dob < today && dob > hundredYearsAgo;
    }, "DOB must be a valid date in the past and no more than 100 years old.");

    // Link it to unobtrusive validation
    $.validator.unobtrusive.adapters.add("validdob", function (options) {
        options.rules["validdob"] = true;
        options.messages["validdob"] = options.message;
    });
});
