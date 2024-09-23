"use strict";
function AuthResetPassword(localizer) {
    const form = document.querySelector("#kt_password_reset_form")
    const submitButton = document.querySelector("#kt_password_reset_submit")

    // Initialize form validation
    const validator = FormValidation.formValidation(form, {
        fields: {
            email: {
                validators: {
                    regexp: {
                        regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                        message: localizer['EmailInvalid'],
                    },
                    notEmpty: {
                        message: localizer['EmailRequired'],
                    }
                }
            }
        },
        plugins: {
            trigger: new FormValidation.plugins.Trigger(),
            bootstrap: new FormValidation.plugins.Bootstrap5({
                rowSelector: ".fv-row",
                eleInvalidClass: "",
                eleValidClass: ""
            })
        }
    });

    // Handle error
    function handleSwalError() {
        Swal.fire({
            text: localizer['Swal.Error'],
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: localizer['OkGotIt'],
            customClass: {
                confirmButton: "btn btn-primary"
            }
        });
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
    }

    attachEventListeners()
}

KTUtil.onDOMContentLoaded(function () {
    loadLocalization().then((localizer) => {
        AuthResetPassword(localizer)
    });
})