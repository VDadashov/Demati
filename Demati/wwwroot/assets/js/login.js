//#region loginPasswordEye
function loginpasswordEyeTwo(){
    const passwordEye = document.querySelector('#password-eye');
    const passwordInput = document.querySelector('#loginPassword');
  
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
  loginpasswordEyeTwo();
//#endregion

function loginReset(){
  let resetPasswordBtn = document.querySelector('#login-resetBtn');
  let resetCloseBtn = document.querySelector('#login-resetClose');
  let loginContent = document.querySelector('.login__content__one');
  let resetContent = document.querySelector('.login__reset-password');

  resetPasswordBtn.addEventListener('click',()=>{
    loginContent.style.display = 'none';
    resetContent.style.display = 'inline-block';
  })

  resetCloseBtn.addEventListener('click',()=>{
    resetContent.style.display = 'none';
    loginContent.style.display = 'flex';
  })
}
loginReset();