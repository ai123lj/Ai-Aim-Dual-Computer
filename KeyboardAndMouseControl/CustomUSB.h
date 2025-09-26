#pragma once

#include "USB.h"

// 自定义USB类，继承自ESPUSB，可以自定义USB参数
class CustomUSBClass : public ESPUSB {
public:
  CustomUSBClass(size_t event_task_stack_size = 2048, uint8_t event_task_priority = 5);
  bool begin();
  
private:
  void initCustomParams();
};

// 全局实例
extern CustomUSBClass CustomUSB;