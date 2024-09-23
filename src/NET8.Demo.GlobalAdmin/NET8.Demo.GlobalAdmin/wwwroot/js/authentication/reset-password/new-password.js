"use strict";
function AuthNewPassword(localizer) {
    const form = document.querySelector("#kt_new_password_form");
    const submitButton = document.querySelector("#kt_new_password_submit");
    const passwordMeter = KTPasswordMeter.getInstance(form.querySelector('[data-kt-password-meter="true"]'));
    const isPasswordStrong = () => passwordMeter.getScore() > 50;

    // Initialize form validation
    const validator = FormValidation.formValidation(form, {
        fields: {
            password: {
                validators: {
                    notEmpty: {
                        message: localizer['PasswordRequired'],
                    },
                    callback: {
                        message: localizer['PasswordInvalid'],
                        callback: (field) => field.value.length > 0 && isPasswordStrong()
                    }
                }
            },
            "confirmPassword": {
                validators: {
                    notEmpty: {
                        message: localizer['Register.ConfirmPasswordRequied'],
                    },
                    identical: {
                        compare: () => form.querySelector('[name="password"]').value,
                        message: localizer['Register.ConfirmPasswordInvalid'],
                    }
                }
            },
        },
        plugins: {
            trigger: new FormValidation.plugins.Trigger({
                event: {
                    password: false
                }
            }),
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

        form.querySelector('input[name="password"]').addEventListener("input", (event) => {
            if (event.target.value.length > 0) {
                validator.updateFieldStatus("password", "NotValidated");
            }
        });
    }

    attachEventListeners()
}

KTUtil.onDOMContentLoaded(function () {
    loadLocalization().then((localizer) => {
        AuthNewPassword(localizer)
    });
})