@echo off

REM Python 스크립트를 실행하여 엑셀을 JSON으로 변환
python excel_to_json.py BuildingData.xlsx BuildingData.json
python excel_to_json.py ItemData.xlsx ItemData.json
python excel_to_json.py RecipyData.xlsx RecipyData.json
python excel_to_json.py RecipyGroupData.xlsx RecipyGroupData.json
python excel_to_json.py TextData.xlsx TextData.json

pause