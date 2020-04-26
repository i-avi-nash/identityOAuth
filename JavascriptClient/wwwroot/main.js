var config = {
    authority: 'https://localhost:44372/',
    client_id: 'client_id_js',
    redirect_uri: 'https://localhost:44384/Home/SignIn',
    response_type: 'id_token token',
    scope: 'openid ApiOne'
};

var userManager = new Oidc.UserManager(config);

var signIn = function () {
    userManager.signinRedirect();
};

userManager.getUser().then(user => {
    console.log(user);
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    }
});

var callApi = function () {
    axios.get("https://localhost:44324/secret")
        .then(res => {
            console.log(res);
        });
};