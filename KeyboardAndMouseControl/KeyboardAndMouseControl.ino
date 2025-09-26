#include "CustomUSB.h"
#include "USBMouse.h"
CustomHIDDevice Device;

void setup() {
  Serial.begin(2000000);
  Device.begin();
  CustomUSB.begin();
}
char buf[50];const unsigned long TIMEOUT_US = 1000;  // 1毫秒超时
void loop() {
  // 精确接收策略：接收到{后持续等待}
  if (Serial.available() <= 0)
    return;
  // 验证头
  if (Serial.read() != '{') {
    // 清空缓冲区避免数据干扰
    while (Serial.available() > 0) Serial.read();
    return;
  }

  // 进入精确接收模式
  unsigned long startTime = micros();
  
  int len = 0;
  bool foundEnd = false;
  // 持续等待直到接收到}或超时
  while ((micros() - startTime) < TIMEOUT_US && len < sizeof(buf) - 1) {
    if (Serial.available() > 0) {
      char c = Serial.read();
      if (c == '}') {
        foundEnd = true;
        break;  // 找到结束符，退出循环
      }
      buf[len++] = c;
    }
  }
  // 检查接收结果
  if (!foundEnd || len <= 0) {
    // 超时或未找到结束符，清空数据并退出
    while (Serial.available() > 0) Serial.read();  // 清空缓冲区
    Serial.print("{timeout}");
    return;
  }

  buf[len] = '\0';  // 确保字符串结束

  // 数据解析验证
  int mode = 0, x = 0, y = 0;
  if (sscanf(buf, "%d,%d,%d", &mode, &x, &y) != 3) {
    Serial.print("{error}");
    return;
  }
  switch (mode) {
    case 0:
      Device.sendMouseReport(0, x, y, 0);  //只移动鼠标
      break;
    case 1:
      Device.sendMouseReport(MOUSE_LEFT, x, y, 0);  //移动鼠标并射击
      Device.sendMouseReport(0, 0, 0, 0);           //移动鼠标并射击
      break;
    case 2:
      Device.sendMouseReport(MOUSE_LEFT, 0, 0, 0);  //
      break;
    case 3:
      Device.sendMouseReport(0, 0, 0, 0);  //
      break;
    case 4:
      Device.sendMouseReport(MOUSE_LEFT, x, y, 0);  //移动鼠标并射击
      delay(35);
      Device.sendMouseReport(0, 0, 0, -1);  //移动鼠标并射击
      break;
    case 5:
      break;
  }
  Serial.print("{ok}");
}
