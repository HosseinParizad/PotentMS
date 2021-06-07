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
  cats: any[] = []
  totalAngularPackages: string = '';
  hidden = false;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.http.get<any>('https://localhost:5007/PersonalAssistant/Me')
      .subscribe(data => {
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