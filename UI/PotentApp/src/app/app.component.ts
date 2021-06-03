import { Injectable } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'Potent-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

@Injectable()
export class AppComponent implements OnInit {
  title = 'PotentApp';
  totalAngularPackages: string = '';
  headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*');

  constructor(private http: HttpClient) { }

  ngOnInit() {
    // Simple GET request with response type <any>
    // const httpOptions = {
    //   headers: new HttpHeaders({
    //     'Content-Type': 'application/json',
    //     'Access-Control-Allow-Origin': '*',
    //     'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PUT, PATCH, DELETE',
    //     'Access-Control-Allow-Credentials': 'true'
    //   })
    //   , withCredentials: true
    // };

    this.http.get<any>('https://localhost:5007/PersonalAssistant/All')
      .subscribe(data => {
        //this.totalAngularPackages = data;
        console.log('horaaaaaa');
        console.log(data);
      })
    // headers:
    // {
    //   'Content-Type': 'application/json',
    //   'Access-Control-Allow-Origin': '*',
    //   'Access-Control-Allow-Methods': 'GET'
    // }
  }
}