export class Utilities {
    public hasAccessToken(): boolean {
        let token = localStorage.getItem('access_token');
        return token != null && token != '';
    }

    public getAccessToken(): string {
        let token = localStorage.getItem('access_token');
        return token != null ? token : "";
    }

    public deleteAccessToken(): void {
        localStorage.setItem("access_token", '');
    }
}
