deps_config := \
	E:\ProgramData\ESP32\esp-idf\components\app_trace\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\aws_iot\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\bt\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\driver\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\esp32\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\esp_adc_cal\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\esp_event\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\esp_http_client\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\esp_http_server\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\ethernet\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\fatfs\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\freemodbus\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\freertos\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\heap\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\libsodium\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\log\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\lwip\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\mbedtls\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\mdns\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\mqtt\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\nvs_flash\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\openssl\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\pthread\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\spi_flash\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\spiffs\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\tcpip_adapter\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\unity\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\vfs\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\wear_levelling\Kconfig \
	E:\ProgramData\ESP32\esp-idf\components\bootloader\Kconfig.projbuild \
	E:\ProgramData\ESP32\esp-idf\components\esptool_py\Kconfig.projbuild \
	E:\ProgramData\ESP32\esp-idf\components\partition_table\Kconfig.projbuild \
	/e/ProgramData/ESP32/esp-idf/Kconfig

include/config/auto.conf: \
	$(deps_config)

ifneq "$(IDF_TARGET)" "esp32"
include/config/auto.conf: FORCE
endif
ifneq "$(IDF_CMAKE)" "n"
include/config/auto.conf: FORCE
endif

$(deps_config): ;
