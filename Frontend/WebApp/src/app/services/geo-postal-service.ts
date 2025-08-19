import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { PostalCodeAndPlaceDto } from '../dtos/postal-code-and-place-dto';

@Injectable({
  providedIn: 'root'
})
export class GeoPostalService {

  private apiUrl = "https://localhost:7267/api/geopostals/";
  private http = inject(HttpClient);


  getCountries() {
    return this.http.get<string[]>(this.apiUrl + "getcountries");
  }

  getStates(country: string) {
    return this.http.get<string[]>(this.apiUrl + "getstates?country=" + country);
  }

  getPostlCodesAndPlaces(state: string) {
    return this.http.get<PostalCodeAndPlaceDto[]>(this.apiUrl + "getpostalcodesandplaces?state=" + state);
  }


}
