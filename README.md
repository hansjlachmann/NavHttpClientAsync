# Nav http client async
Send async http requests from NAV

TODO Requirements:
- the project must create a .dll file that can be used in Dynamics NAV Classic client
- Must have a Function to execute http request. The function must be available from NAV
- http reques must be async, must test from NAV client that it does not block normal operations
- Parameters:
    - HTTP headers (autorizaion, conent type)
    - endpoint URL
    - Method GET, PUT, POST, DELETE
    - Body
- Function must return text (max 1024) characters (limitations in NAV Classic) - maybe this return valiue is not needed, the point is not to wait for the function call, just execute it


- Register with regasm command `regasm /codebase /tlb NavHttpClientAsync.dll`