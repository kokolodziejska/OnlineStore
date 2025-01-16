// Override the default number validator to accept commas and dots
jQuery.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?\d+([.,]\d+)?$/.test(value);
};

// Override the default range validator to handle commas and dots
jQuery.validator.methods.range = function (value, element, param) {
    // Replace comma with dot to standardize the decimal separator
    var globalizedValue = value.replace(",", ".");
    // Attempt to parse the standardized value
    return this.optional(element) || (parseFloat(globalizedValue) >= param[0] && parseFloat(globalizedValue) <= param[1]);
};
