ping 127.0.0.1 -n 6 > nul
start "chrome" "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --kiosk https://localhost:5001/kanban/line?lineNo=W01L01
exit

###
###  只能使用chrome 79及其以下版本才可以，在2020年1月以后的版本就用不了了
###
