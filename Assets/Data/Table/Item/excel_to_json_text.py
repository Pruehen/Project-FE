import pandas as pd
import sys
import os
import json

def excel_to_custom_json(input_file, output_file):
    try:
        # 첫 번째 줄을 건너뛰고 엑셀 파일을 읽기
        df = pd.read_excel(input_file, skiprows=[1])

        # 결과를 저장할 dictionary 생성
        result = {"dic": {}}

        # 각 행을 순회하며 JSON 구조에 맞게 변환
        for _, row in df.iterrows():
            id = row["Id"]
            result["dic"][id] = {
                "Id": id,
                "Text_Kr": row["Text_Kr"]
            }

        # 결과를 JSON 파일로 저장
        with open(output_file, "w", encoding="utf-8") as json_file:
            json.dump(result, json_file, ensure_ascii=False, indent=4)

        print(f"File converted successfully: {output_file}")

    except Exception as e:
        print(f"Error occurred: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python excel_to_custom_json.py <input_excel_file> <output_json_file>")
    else:
        input_file = sys.argv[1]
        output_file = sys.argv[2]
        excel_to_custom_json(input_file, output_file)