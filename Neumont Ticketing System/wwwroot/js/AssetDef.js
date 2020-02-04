$(document).ready(() => {
    var typesList = $('#typesList');

    var phoneNumberContainers = $('#manufacturersList').find('div').filter(function() {
        return this.id.match(/mfr\d+_PhoneNumbers/);
    });

    var emailAddressContainers = $('#manufacturersList').find('div').filter(function() {
        return this.id.match(/mfr\d+_EmailAddresses/);
    });

    console.log(`Email address containers length: ${emailAddressContainers.length}`);
});