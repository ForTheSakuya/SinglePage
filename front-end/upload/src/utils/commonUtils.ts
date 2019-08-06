namespace commonUtils {
    /**
     * 获取url地址参数
     */
    export let getUrlQueryParam = (key: string, notToLower?: boolean): string | undefined => {
        key = (!notToLower ? key.toLowerCase() : key).replace(/[\[\]]/g, '\\$&');
        const regex: RegExp = new RegExp('[?&]' + key + '(=([^&#]*)|&|#|$)');
        const results: RegExpExecArray | null = regex.exec(!notToLower ? window.location.href.toLowerCase() : window.location.href);
        if (!results) {
            return undefined;
        }
        if (!results[2]) {
            return '';
        }
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }
    
    /**
     * 添加url参数
     *
     * @param url 原url
     * @param param 字符串，'a=123'
     * @return 已拼接参数的url字符串
     */
    export let addUrlParam = (url: string | undefined, param: string) => {
        if (!url) {
            return '';
        }
        url += (url.indexOf('?') !== -1 ? '&' : '?') + param;
        return url;
    }
}

export default commonUtils