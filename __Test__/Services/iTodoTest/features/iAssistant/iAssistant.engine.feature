Feature: iAssistant
	Assistant main engine 

Scenario: See task need to do
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |
	Then Asstant should ask me to do following tasks:
		| Id                                   | Text                                | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |