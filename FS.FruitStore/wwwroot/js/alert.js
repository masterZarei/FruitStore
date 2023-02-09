let alertBox;
let closeBtn;


function showAlert(State, Text) {
    let states = ['info', 'success', 'danger', 'warning'];
    let currentState;

    for (let i = 0; i < states.length; i++) {
        if(State == states[i]){
            currentState = i;
        }
    }
    if (currentState != undefined) {
        let MotherDiv = document.createElement('div');
        MotherDiv.classList = 'alert-box show ' + states[currentState];
        alertBox = MotherDiv;

        let newContent = document.createTextNode(Text)
        MotherDiv.appendChild(newContent);

        var currentDiv = document.querySelector('.alert-box');
        document.body.appendChild(MotherDiv, currentDiv);

        closeBtn = document.createElement('span');
        closeBtn.classList = 'close-btn';
        
        icon = document.createElement('i');
        icon.classList = 'fas fa-times ';
        closeBtn.appendChild(icon)
        closeBtn.addEventListener('click', ()=> {
            hideAlertBox();
        });

        MotherDiv.appendChild(closeBtn);
        document.body.appendChild(MotherDiv, closeBtn);


        showAlertBox();
        return true;
    } else {
        showError();
        return false;
    }

}


function hideAlertBox() {
    alertBox.classList.remove('show')
    alertBox.classList.add('hide')
}


function showAlertBox() {
    if (alertBox.classList.contains('hidden')) {
        alertBox.classList.remove('hidden');
    };
    

   let timer = setTimeout(() => {
        hideAlertBox();
    }, 5000);
}

function showError() {
    showAlert('danger', 'خطا');
}












