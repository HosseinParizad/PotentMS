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
    //this.http.get<any>('https://localhost:5007/PersonalAssistant/AllofUs')
    this.http.get<any>('https://localhost:5007/PersonalAssistant/' + this.group)
      .subscribe(data => {
        this.selected.badge = {};
        this.cats = [];
        //data.sort((a: any, b: any) => a.id == this.selected.group ? 0 : 1).forEach((row: any) => {
        data.forEach((row: any) => {
          this.cats.push(row);
          if (this.selected.group == undefined) {
            this.selected.group = row.id;
          }
        });
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
    var body = this.BodyMaker('newTask', this.selected.group, { Description: this.text, ParentId: "" });
    this.sent = this.SendRequest(body);
    return false;
  }

  SendMemoryRequest() {
    var body = this.BodyMaker('newMemory', this.selected.group, { Text: this.text, Hint: this.description, ParentId: "" });
    this.sent = this.SendRequestCore("/Memory", body);
    return false;
  }

  SendTaskRequestSpe(body: string) {
    if (body.substring(0, 4) == 'http') {
      window.open(body, "_blank");
    }
    else {
      body = body.replace('[text]', this.text)
      body = body.replace('[date]', this.inputdate)
      this.sent = this.SendRequest(JSON.parse(body));
    }
  }

  SendGoalRequest() {
    var body = this.BodyMaker('newGoal', this.selected.group, { Description: this.text, ParentId: "" });
    this.sent = this.SendRequest(body);
    return false;
  }

  SendGroupRequest() {
    var body = this.BodyMaker('newGroup', this.selected.group, { Description: this.text, ParentId: "" });
    this.sent = this.SendRequest(body);
    return false;
  }

  SendMemberRequest() {
    var body = this.BodyMaker('newMember', this.group, { NewMember: this.text });
    this.sent = this.SendRequest(body);
    return false;
  }

  SendReapeatRequest(frequency: number) {
    var body = this.BodyMaker('registerRepeat', this.selected.group, { ReferenceId: this.selected.badge.id, Frequency: frequency, ReferenceName: "Task", RepeatIfAllClosed: this.repeatIfAllClosed });
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

  moveTo(member: string, location: string) {
    var content = { Member: member, Location: location };
    var url = "https://localhost:5001/Gateway/Location";
    var body = { action: "setCurrentLocation", key: member, content: JSON.stringify(content) };
    this.SendRequestCore(url, body);
  }
}