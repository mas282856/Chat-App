# Chat-App
## 概要
- 趣味マッチングアプリ
- 狙い
  - 趣味を持つ人々がつながり楽しさや情報を共有できるプラットフォームを提供すること
  - チャットでの交流を通じて面白いアイディアが生まれる環境を作ること


## 使用技術
- Unity 2022.3.5f1

- AWS
  - DynamoDB
  - AWS AppSync
  - AWS SDK for .NET
  
- SQLite<br>
## 機能
- ロビー画面
  - チャット概要の表示
- マッチング画面
  - マッチング情報の入力（ニックネーム、年齢、性別、性格、職業、出身）
- チャット画面
  - テキスト送る
　- 制限時間の表示
  - タイトルの設定

![image](https://github.com/mas282856/Chat-App/assets/134497959/871dd5aa-2c15-45d3-9b34-5a6ffab3f9e9)


## AWS構成図
![image](https://github.com/mas282856/Chat-App/assets/134497959/cd19e51c-97f2-467d-a332-e46f2c409d9c)

## 担当箇所
- ロビー画面
  - [Assets/Scripts/HomeView/](https://github.com/mas282856/Chat-App/tree/main/Assets/Scripts/HomeView)
- チャット画面
  - [Assets/Scripts/ChatView/](https://github.com/mas282856/Chat-App/tree/main/Assets/Scripts/ChatView)
- その他
  - [Assets/Scripts/AWS/DDBChatChatOverview.cs (AWS SDK for .NETによるDynamoDBのデータ操作)](https://github.com/mas282856/Chat-App/tree/main/Assets/Scripts/SQLite)


