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
  cats:any[] = []
  totalAngularPackages: string = '';
  hidden = false;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    // Simple GET request with response type <any>
    // const httpOptions = {
    //   headers: new HttpHeaders({
    //     'Content-Type': 'application/json',
    //     'Access-Control-Allow-Origin': '*',
    //     'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PUT, PATCH, DELETE',
    //   })
    // };

    this.http.get<any>('https://localhost:5007/PersonalAssistant/Me')
      .subscribe(data => {
        //this.totalAngularPackages = data;
        //console.log(data);
        data.forEach((row: any) => {
          console.log(row);
          this.cats.push(row)
        });
      })
  }

  toggleBadgeVisibility() {
    this.hidden = !this.hidden;
  }
}