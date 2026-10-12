import { AbstractControl, ValidationErrors } from '@angular/forms';

/** Como `Validators.required`, mas também rejeita texto composto só de espaços. */
export function textoObrigatorio(controle: AbstractControl<string>): ValidationErrors | null {
  return typeof controle.value === 'string' && controle.value.trim() !== ''
    ? null
    : { required: true };
}
