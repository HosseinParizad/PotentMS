Feature: PersonalAssistant

	Scenario: Default Borad
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |

	Scenario: When we addd task Borad should catch the event
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |
		Given I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following board:
			| AssistantKey | Text | Badges        |
			| Ali          | Goal | ["Deadlines"] |
			| Ali          | Tag  | ["Home"]      |
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following board:
			| AssistantKey | Text | Badges        |
			| Ali          | Goal | ["Deadlines"] |
			| Ali          | Tag  | ["Home"]      |
		When User add 'Work' to tag 0 on selected task for 'Ali'
		Then I should see the following board:
			| AssistantKey | Text | Badges          |
			| Ali          | Goal | ["Deadlines"]   |
			| Ali          | Tag  | ["Home","Work"] |
		When User add 'One,One' to tag 0 on selected task for 'Ali'
		Then I should see the following board:
			| AssistantKey | Text | Badges                |
			| Ali          | Goal | ["Deadlines"]         |
			| Ali          | Tag  | ["Home","Work","One"] |

	Scenario: When use see deadline show see task be deadline order
		Then I should see the following board:
			| AssistantKey | Text | Badges        |
			| Ali          | Goal | ["Deadlines"] |
			| Ali          | Tag  | []            |
		Given I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
			| Watch dog videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set deadline '2021-10-07T00:00:00Z' on selected task for 'Ali'
		When User select item 2 from tasks of 'Ali'
		When User set deadline '2021-10-06T00:00:00Z' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Deadline                  |
			| Watch cat videos on YouTube all day | Ali      | 2021-10-07T00:00:00+00:00 |
			| Watch dog videos on YouTube all day | Ali      | 2021-10-06T00:00:00+00:00 |
		Then I should see the following board deallines:
			| GroupKey | Text                                | Deadline                  |
			| Ali      | Watch dog videos on YouTube all day | 2021-10-06T00:00:00+00:00 |
			| Ali      | Watch cat videos on YouTube all day | 2021-10-07T00:00:00+00:00 |
		Given I send the following task:
			| TaskDesc                               | GroupKey |
			| Watch donkey videos on YouTube all day | Ali      |
		When User select item 3 from tasks of 'Ali'
		When User set deadline '2021-10-01T00:00:00Z' on selected task for 'Ali'
		Then I should see the following board deallines:
			| GroupKey | Text                                   | Deadline                  |
			| Ali      | Watch donkey videos on YouTube all day | 2021-10-01T00:00:00+00:00 |
			| Ali      | Watch dog videos on YouTube all day    | 2021-10-06T00:00:00+00:00 |
			| Ali      | Watch cat videos on YouTube all day    | 2021-10-07T00:00:00+00:00 |

