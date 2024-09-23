'use strict'

function SigninGeneral(localizer) {
    const form = document.querySelector("#kt_sign_in_form")
    const submitButton = document.querySelector("#kt_sign_in_submit")
    const resendEmailLink = document.querySelector("#resend_confirmation_email_link");

    // Initialize form validation
    const validator = FormValidation.formValidation(form, {
        fields: {
            userName: {
                validators: {
                    notEmpty: {
                        message: localizer['UserNameRequired'],
                    },
                },
            },
            password: {
                validators: {
                    notEmpty: {
                        message: localizer['PasswordRequired'],
                    },
                },
            },
        },
        plugins: {
            trigger: new FormValidation.plugins.Trigger(),
            bootstrap: new FormValidation.plugins.Bootstrap5({
                rowSelector: '.fv-row',
                eleInvalidClass: '',
                eleValidClass: '',
            }),
        },
    })

    // Handle login error
    function handleSwalError(message) {
        Swal.fire({
            text: message ?? localizer['Swal.Error'],
            icon: 'error',
            buttonsStyling: false,
            confirmButtonText: localizer['OkGotIt'],
            customClass: {
                confirmButton: 'btn btn-primary',
            },
        })
    }

    // Handle login success
    function handleSwalSuccess(message) {
        Swal.fire({
            text: message,
            icon: 'success',
            buttonsStyling: false,
            confirmButtonText: localizer['OkGotIt'],
            customClass: {
                confirmButton: 'btn btn-primary',
            },
        })
    }

    function disableButtonTemporarily(e, timeout) {
        e.setAttribute('data-kt-indicator', 'on')
        e.disabled = true
        setTimeout(function () {
            e.removeAttribute("data-kt-indicator")
            e.disabled = false
        }, timeout);
    }

    async function resendConfirmationEmail(e) {
        disableButtonTemporarily(e, 60000);

        try {
            const email = e.getAttribute('data-email');
            const returnUrl = e.getAttribute('data-return-url');

            const response = await fetch('/Account/ResendConfirmationEmail', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({
                    email: email,
                    returnUrl: returnUrl
                })
            });

            if (!response.ok) {
                let { error } = await response.json()
                handleSwalError(error.message)
            }

            let result = await response.json()
            handleSwalSuccess(result.message)

        } catch (error) {
            console.log(error)
            handleSwalError()
        } finally {
            e.removeAttribute("data-kt-indicator")
            e.disabled = false
        }
    }

    // Handle form submission
    function attachEventListeners() {
        submitButton.addEventListener('click', async function (event) {
            event.preventDefault()
            const result = await validator.validate()

            if (result === 'Valid') {
                submitButton.setAttribute('data-kt-indicator', 'on')
                submitButton.disabled = true
                form.submit()
            } else {
                handleSwalError()
            }
        })

        if (resendEmailLink) {
            resendEmailLink.addEventListener('click', function (event) {
                event.preventDefault();
                resendConfirmationEmail(this);
            });
        }
    }

    attachEventListeners()
}

KTUtil.onDOMContentLoaded(function () {
    loadLocalization().then((localizer) => {
        SigninGeneral(localizer)
    });
})