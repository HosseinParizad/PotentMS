import { Injectable } from '@angular/core';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { stringify } from '@angular/compiler/src/util';

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
  description: string = "";
  group: string = "Family";
  selected: any = {};
  selectedPart: any = {};
  sent = {};
  isShowdetail: boolean = false;
  inputdate: any;
  repeatIfAllClosed: boolean = false;
  frequency: any = {
    daily: 'D1',
    weekly: 'D7',
    days10: 'D10',
    fortnightly: 'D14',
    monthly: 'M1',
    test: 'T0'
  }

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.http.get<any>('https://localhost:5007/PersonalAssistant/' + this.group)
      .subscribe(data => {
        this.selected.Badge = {};
        this.cats = [];
        data.forEach((row: any) => {
          this.addSection(row, "Memorizes", 'https://localhost:5008/Memory/GetPresentation?groupKey=');
          this.addSection(row, "Goal", 'https://localhost:5010/Goal/GetPresentation?groupKey=');

          this.addSection(row, "Task", 'https://localhost:5003/TodoQuery/GetPresentationTask?groupKey=');

          this.cats.push(row);
          if (this.selected.Group == undefined) {
            this.selected.Group = row.Id;
          }
        });
      });
  }

  addSection(row: any, group: string, url: string) {
    this.http.get<any>(url + row.Id)
      .subscribe(data => {
        setTimeout(() => {
          row.Parts.filter((i: any) => i.Text == group)[0].Badges = data;
        }, 1000);
      });
  }

  showDetail(part: any) {
    this.isShowdetail = !this.isShowdetail;
    this.selectedPart = part;
  }

  toggleBadgeVisibility() {
    this.hidden = !this.hidden;
  }

  setdate(days: number) {
    var date = new Date();
    var date = new Date(date.setDate(date.getDate() + days))
    this.inputdate = date.toISOString().substring(0, 10);

    var badge = this.selected.badge;
    if (badge) {
      for (const key in badge.linkItems) {
        if (Object.prototype.hasOwnProperty.call(badge.linkItems, key)) {
          const e = badge.linkItems[key];
          if (e.text == 'Deadline') {
            this.SendTaskRequestSpe(e.link);
          }
        }
      }
    }
  }

  BodyMaker(action: string, groupKey: string, content: any) {
    return { Action: action, Metadata: { GroupKey: groupKey }, Content: content };
  }

  SendTaskRequest() {
    var body = this.BodyMaker('newTask', this.selected.Group, { Description: this.text, ParentId: "" });
    this.sent = this.SendRequest(body);
    return false;
  }

  SendMemoryRequest() {
    var body = this.BodyMaker('newMemory', this.selected.Group, { Text: this.text, Hint: this.description, ParentId: "" });
    this.sent = this.SendRequestCore("/Memory", body);
    return false;
  }

  SendGoalRequest() {
    var body = this.BodyMaker('newGoal', this.selected.Group, { Text: this.text, ParentId: "" });
    this.sent = this.SendRequestCore("/Goal", body);
    return false;
  }

  SendTaskRequestSpe(body: any) {
    if (body.Content.Text) {
      body.Content.Text = body.Content.Text.replace('[text]', this.text)
      body.Content.Text = body.Content.Text.replace('[date]', this.inputdate)
    }
    if (body.Content.Hint) {
      body.Content.Hint = body.Content.Hint.replace('[hint]', this.description)
    }

    this.sent = this.SendRequestCore(body.Group, body);
  }

  SendGroupRequest() {
    this.selected.Group = this.text;
    var body = this.BodyMaker('newGroup', this.selected.Group, { Group: this.text });
    this.sent = this.SendRequestCore("/Group", body);
    return false;
  }

  SendMemberRequest() {
    var body = this.BodyMaker('newMember', this.group, { NewMember: this.text });
    this.sent = this.SendRequestCore("/Group", body);
    return false;
  }

  SendReapeatRequest(frequency: number) {
    var body = this.BodyMaker('registerRepeat', this.selected.Group, { ReferenceId: this.selected.Badge.Id, Frequency: frequency, ReferenceName: "Task", RepeatIfAllClosed: this.repeatIfAllClosed });
    this.sent = this.SendRequestCore("/Repeat", body);
    return false;
  }

  Reset() {
    var url = "https://localhost:5001/Gateway/Common";
    var body = { action: "reset", key: "Do not care", content: {} };
    this.SendRequestCore(url, body);
  }

  DeleteAll() {
    var url = "https://localhost:5001/Gateway/DeleteTopics";
    var body = {};
    this.SendRequestCore(url, body);
  }

  DeleteFeedback() {
    var url = "https://localhost:5001/Gateway/DeleteFeedback";
    var body = {};
    this.SendRequestCore(url, body);
  }

  SendRequest(body: any) {
    return this.SendRequestCore("", body)
  }

  SendRequestCore(url: string, body: any) {
    const headers = new HttpHeaders()
    // headers.append('Content-Type', 'application/json')
    // headers.append('Access-Control-Allow-Origin', '*')
    // headers.append('Access-Control-Allow-Credentials', 'true')
    // headers.append('Access-Control-Allow-Methods', 'GET, POST, PATCH, DELETE, PUT, OPTIONS')
    // headers.append('Access-Control-Allow-Headers', 'Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With')
    headers.append('Accept', 'application/json')
    if (url.substr(0, 4) != 'http') {
      url = 'https://localhost:5001/Gateway' + url
    }
    this.http.post<any>(url, body, { headers }).subscribe({
      next: data => {
        this.text = '';
        setTimeout(() => {
          this.ngOnInit();
        }, 1000);
      },
      error: error => {
        alert(error.message);
      }
    })
    return body;
  }

  selectgroup(group: string) {
    this.selected.Group = group;
    return false;
  }

  selectbadgepart(event: any, part: string, badge: any) {
    if (badge.Id) {
      this.selected.Part = part;
      this.selected.Badge = badge;
      this.sent = badge;
    }
    event.stopPropagation();
    return false;
  }

  moveTo(member: string, location: string) {
    var content = { Member: member, Location: location };
    var url = "https://localhost:5001/Gateway/Location";
    var body = { action: "setCurrentLocation", key: member, content: JSON.stringify(content) };
    this.SendRequestCore(url, body);
  }
}