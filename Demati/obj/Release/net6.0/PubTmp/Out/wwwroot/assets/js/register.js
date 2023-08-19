//#region passwordCharacterActive
function registerpasswordEye(){
    const passwordEye = document.querySelector('#password-eye');
    const passwordInput = document.querySelector('.register-password');
  
    passwordEye.addEventListener('click', () => {
      if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        passwordEye.innerHTML = '<i class="fa-regular fa-eye-slash"></i>';
      } else {
        passwordInput.type = 'password';
        passwordEye.innerHTML = '<i class="fa-regular fa-eye"></i>';
      }
    });
  }
  registerpasswordEye();
  //#endregion