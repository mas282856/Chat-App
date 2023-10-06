# Chat-App
## 概要
- 趣味マッチングアプリ
- 狙い…多くの人が見る、面白いコンテンツを生み出しやすい環境を作る

## 使用技術
- Unity 2022.3.5f1

- AWS
  - DynamoDB
  - AWS AppSync
  - AWS SDK for .NET
  
- SQLite<br>
## 機能
- マッチング画面
  - マッチング情報の入力（ニックネーム、年齢、性別、性格、職業、出身）
- ロビー画面
  - チャット概要の表示
- チャット画面
  - テキスト送る
	- 制限時間の表示
  - タイトルの設定

## 担当箇所
- ロビー画面
  - [Assets/Scripts/HomeView/](https://github.com/mas282856/Chat-App/tree/main/Assets/Scripts/HomeView)
- チャット画面
  - [Assets/Scripts/ChatView/](https://github.com/mas282856/Chat-App/tree/main/Assets/Scripts/ChatView)
- その他
  - [Assets/Scripts/AWS/DDBChatChatOverview.cs (AWS SDK for .NETによるDynamoDBのデータ操作)](https://github.com/mas282856/Chat-App/tree/main/Assets/Scripts/SQLite)
