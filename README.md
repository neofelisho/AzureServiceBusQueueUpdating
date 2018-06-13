# AzureServiceBusQueueUpdating

`Problem`
- 目前的專案使用 Azure Service Bus 當做 delayed jobs 的 Message Queue，然而卻發現 Queue 中有重複訊息的問題。
- 已經有設定 Queue 的 `Duplicate detection history`，從 30 秒到 10 分鐘的設定都試過。

`Testing`
- AzureServiceBusEnqueuer:
  - 一個簡單的 Console 程式，會送兩個相同 MessageId 的訊息至指定的 queue 中，理論上會因為重複的 MessageId，第二筆訊息會被濾掉。
  - 但實際上怎麼測試都不會生效，故先取得第一筆 enqueue 的 sequence number，並在 enqueue 第二筆之前，將第一筆刪除。

- AzureServiceBusQueueReceiver:
  - 一個簡單的 WebJob 程式，放到雲端讓 Service Bus Trigger 這隻程式，將 trigger 過來的訊息送至 slack 觀察正確性。
  - 在沒有插入 ```CancelScheduledMessageAsync(sequenceNumber);``` 之前，會一直收到兩筆訊息。
  
`Summary`
- 在找到真正原因之前，會先使用 Redis 暫存每次 enqueue 的 sequence number(s)，並使用訊息的生效時間當做 Redis Message expired time。
