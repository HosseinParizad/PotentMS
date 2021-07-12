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
    this.http.get<any>('https://localhost:5007/PersonalAssistant/AllofUs')
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
    var body = { Action: 'newTask', Key: this.selected.group, Content: JSON.stringify({ Description: this.text, ParentId: "" }) };
    this.sent = this.SendRequest(body);
    return false;
  }

  SendTaskRequestSpe(body: string) {
    body = body.replace('[text]', this.text)
    this.sent = this.SendRequest(JSON.parse(body));
  }

  SendGoalRequest() {
    var body = { Action: 'newGoal', Key: this.selected.group, Content: JSON.stringify({ Description: this.text, ParentId: "" }) };
    this.sent = this.SendRequest(body);
    return false;
  }

  SendGroupRequest() {
    var body = { Action: 'newGroup', Key: this.text, Content: JSON.stringify({ Description: this.text, ParentId: "" }) };
    this.sent = this.SendRequest(body);
    return false;
  }

  SendMemberRequest() {
    var body = { Action: 'newMember', Key: this.selected.group, Content: JSON.stringify({ NewMember: this.text }) };
    this.sent = this.SendRequest(body);
    return false;
  }

  SendRequest(body: any) {
    const headers = new HttpHeaders()
    // headers.append('Content-Type', 'application/json')
    // headers.append('Access-Control-Allow-Origin', '*')
    // headers.append('Access-Control-Allow-Credentials', 'true')
    // headers.append('Access-Control-Allow-Methods', 'GET, POST, PATCH, DELETE, PUT, OPTIONS')
    // headers.append('Access-Control-Allow-Headers', 'Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With')
    headers.append('Accept', 'application/json')

    this.http.post<any>('https://localhost:5001/Gateway', body, { headers }).subscribe({
      next: data => {
        this.text = '';
        setTimeout(() => {
          this.ngOnInit();
        }, 100);

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

  selectbadgepart(event: any, part: string, badge: any) {
    if (badge.id) {
      this.selected.part = part;
      this.selected.badge = badge;
      this.sent = badge;
    }
    event.stopPropagation();
    return false;
  }
}