# AzureServiceBusQueueUpdating
###### tags: `Azure` `WebJobs` `Service Bus` `Message Queue` `Duplicate Detection`

`Problem`
- 目前的專案使用 Azure Service Bus 當做 delayed jobs 的 Message Queue，然而卻發現 Queue 中有重複訊息的問題。
- 已經有設定 Queue 的 `Duplicate detection history`，從 30 秒到 10 分鐘的設定都試過。

`Testing`
- AzureServiceBusEnqueuer:
  - 一個簡單的 Console 程式，會送兩個相同 MessageId 的訊息至指定的 queue 中，理論上會因為重複的 MessageId，第二筆訊息會被濾掉。
  ~~- 但實際上怎麼測試都不會生效，故先取得第一筆 enqueue 的 sequence number，並在 enqueue 第二筆之前，將第一筆刪除。~~
  - 原先不生效的問題是發生在 Azure Portal 上面設定的 bug，`Duplicate detection`選項必須在建立 Queue 時就先開啟，事後開啟是無效的。
  - 然而依照我們使用的 scenario，`detection window`必須設定成 job cron 的時間，我們 job 排程是一小時一次，而 detection window 設定到一小時的話，service bus 會有嚴重效能的問題，message trigger 會嚴重 delay。

- AzureServiceBusQueueReceiver:
  - 一個簡單的 WebJob 程式，放到雲端讓 Service Bus Trigger 這隻程式，將 trigger 過來的訊息送至 slack 觀察正確性。
  - 在沒有插入 ```CancelScheduledMessageAsync(sequenceNumber);``` 之前，會一直收到兩筆訊息。
  
`Summary`
~~- 在找到真正原因之前，會先使用 Redis 暫存每次 enqueue 的 sequence number(s)，並使用訊息的生效時間當做 Redis Message expired time。~~
- 導致不生效的原因如上所述，然而依照我們系統的情境，還是使用 Redis 自己實作 duplicate detection 才能取得效能上的優勢。
