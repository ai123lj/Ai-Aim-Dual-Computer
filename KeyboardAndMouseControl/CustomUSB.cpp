#include "CustomUSB.h"

CustomUSBClass::CustomUSBClass(size_t event_task_stack_size, uint8_t event_task_priority) 
  : ESPUSB(event_task_stack_size, event_task_priority) {
  initCustomParams();
}

void CustomUSBClass::initCustomParams() {
  // 模仿Logitech LIGHTSPEED Receiver的USB参数配置
  // 基于USB设备信息：VID_046D&PID_C53F
  VID(0x046D);                        // Logitech Inc. 厂商ID
  PID(0xC53F);                        // LIGHTSPEED Receiver 产品ID
  productName("USB Receiver");         // 产品名称
  manufacturerName("Logitech");        // 制造商名称
  serialNumber("RQR55.01_B0008");      // 配置序列号
  usbVersion(0x0200);                 // USB 2.0 版本
  usbPower(98);                       // 功耗 98mA
  usbClass(0x03);                     // HID设备类
  usbSubClass(0x01);                  // Boot Interface子类
  usbProtocol(0x02);                  // Mouse协议
  usbAttributes(0xA0);                // 支持远程唤醒，非自供电
}

bool CustomUSBClass::begin() {
  // 在开始前确保参数已设置
  initCustomParams();
  return ESPUSB::begin();
}

// 创建全局实例
CustomUSBClass CustomUSB;