import axios from 'axios'
import commonUtils from "./commonUtils";

interface IError {
    code: string
    title: string
}

interface IResponse<T> {
    success: boolean
    data?: T
    error?: IError
}

const http = axios.create({
    baseURL: '',
    withCredentials: true
});

http.interceptors.request.use(config => {
    // 取参数里的token带上
    const token = commonUtils.getUrlQueryParam('token');
    if (token) {
        config.url = commonUtils.addUrlParam(config.url, 'token=' + token);
    }
    return config;
}, error => {
    return Promise.reject(error)
})

export {
    http,
    IResponse
}