function CheckLogin(Permitted_role){
    const token = localStorage.getItem('token');
    if (token){
        const role = decodeJwt(token).role
        if (role != Permitted_role){
        window.location.href ='/home/login'
        }
    }
    else{
        window.location.href ='/home/login'
    }
}