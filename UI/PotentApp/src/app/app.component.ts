import { Injectable } from '@angular/core';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
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
  text: string = "";
  selected: any = {};
  sent = {};

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.http.get<any>('https://localhost:5007/PersonalAssistant/family')
      .subscribe(data => {
        this.selected.badge = {};
        this.cats = [];
        data.forEach((row: any) => {
          this.cats.push(row)
          if (this.selected.group == undefined) {
            this.selected.group = row.id;
          }
        });
      })
  }

  toggleBadgeVisibility() {
    this.hidden = !this.hidden;
  }

  SendTaskRequest() {
    this.sent = this.SendRequest('newTask');
    return false;
  }
  SendTaskRequestSpe(action: string) {
    this.sent = this.SendRequest("", action);
  }

  SendGoalRequest() {
    this.sent = this.SendRequest('newGoal');
    return false;
  }

  SendRequest(action: string, bod: string = "") {
    const headers = new HttpHeaders()
    // headers.append('Content-Type', 'application/json')
    // headers.append('Access-Control-Allow-Origin', '*')
    // headers.append('Access-Control-Allow-Credentials', 'true')
    // headers.append('Access-Control-Allow-Methods', 'GET, POST, PATCH, DELETE, PUT, OPTIONS')
    // headers.append('Access-Control-Allow-Headers', 'Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With')
    headers.append('Accept', 'application/json')
    var body = { Action: action, Key: this.selected.group, Content: JSON.stringify({ Description: this.text, ParentId: "" }) };
    if (bod) {
      bod = bod.replace('[text]', this.text)
      body = JSON.parse(bod);
    }
    this.http.post<any>('https://localhost:5001/Gateway', body, { headers }).subscribe({
      next: data => {
        this.text = '';
        this.ngOnInit();
      },
      error: error => {
        alert(error.message);
      }
    })
    return body;
  }

  selectgroup(group: string) {
    this.selected.group = group;
    return false;
  }

  selectbadgepart(event:any, part: string, badge: any) {
    if (badge.id)
    {
      this.selected.part = part;
      this.selected.badge = badge;
      this.sent = badge;
    }
    event.stopPropagation();
    return false;
  }
}