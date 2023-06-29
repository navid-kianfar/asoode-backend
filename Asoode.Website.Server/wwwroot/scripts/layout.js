(function($, window, document, undefined){
    $(function(){
        $('.collapse').collapse({
            toggle: false
        });
        
        const contactForm = $('#frmContact');
        const contactFormButton = $('#frmContact button');
        const contactFormFirstName = $('#frmContact #txtFirstName');
        const contactFormLastName = $('#frmContact #txtLastName');
        const contactFormEmail = $('#frmContact #txtEmail');
        const contactFormMessage = $('#frmContact #txtMessage');
        if (contactForm && contactForm.length) {
            contactFormButton.on('click',function(event){
                event.preventDefault();
                event.stopPropagation();

                contactFormFirstName.parent().removeClass('error');
                contactFormLastName.parent().removeClass('error');
                contactFormEmail.parent().removeClass('error');
                contactFormMessage.parent().removeClass('error');
                
                let hasError = false;
                const model = {};
                model.firstName = contactFormFirstName.val();
                if (!model.firstName) {
                    hasError = true;
                    contactFormFirstName.parent().addClass('error');
                }
                model.lastName = contactFormLastName.val();
                if (!model.lastName) {
                    hasError = true;
                    contactFormLastName.parent().addClass('error');
                }
                model.email = contactFormEmail.val();
                if (!model.email) {
                    hasError = true;
                    contactFormEmail.parent().addClass('error');
                }
                model.message = contactFormMessage.val();
                if (!model.message) {
                    hasError = true;
                    contactFormMessage.parent().addClass('error');
                }
                
                if (hasError) { return; }

                contactFormButton.attr('disabled', 'true');
                
                $.ajax({
                    url : 'https://api.' + window.location.host + '/v2/contact',
                    type : 'POST',
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    crossDomain: true,
                    success : function(data) {
                        console.log(data);
                        contactFormButton.removeAttr('disabled');
                        if (data.status === 2) {
                            contactFormFirstName.val('');
                            contactFormLastName.val('');
                            contactFormEmail.val('');
                            contactFormMessage.val('');
                        }
                    },
                    error : function(request, error) {
                        contactFormButton.removeAttr('disabled');
                    }
                });
            });
        }
    });
})(jQuery, window, document);