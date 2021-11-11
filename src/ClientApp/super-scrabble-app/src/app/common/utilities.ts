export class Utilities {
    public hasAccessToken(): boolean {
        let token = localStorage.getItem('access_token');
        return token != null && token != '';
    }
}
