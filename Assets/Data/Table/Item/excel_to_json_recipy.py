import pandas as pd
import sys
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
                "Name": row["Name"],
                "Desc": row["Desc"],
                "Icon": row["Icon"].replace("Resources/UI/Icon/Recipy/", "UI/Icon/Recipy/").replace(".png", ""),
                "InputItem-1": row["InputItem-1"] if pd.notna(row["InputItem-1"]) else None,
                "InputItemCount-1": int(row["InputItemCount-1"]) if pd.notna(row["InputItemCount-1"]) else 0,
                "InputItem-2": row["InputItem-2"] if pd.notna(row["InputItem-2"]) else None,
                "InputItemCount-2": int(row["InputItemCount-2"]) if pd.notna(row["InputItemCount-2"]) else 0,
                "InputItem-3": row["InputItem-3"] if pd.notna(row["InputItem-3"]) else None,
                "InputItemCount-3": int(row["InputItemCount-3"]) if pd.notna(row["InputItemCount-3"]) else 0,
                "InputItem-4": row["InputItem-4"] if pd.notna(row["InputItem-4"]) else None,
                "InputItemCount-4": int(row["InputItemCount-4"]) if pd.notna(row["InputItemCount-4"]) else 0,
                "OutputItem-1": row["OutputItem-1"] if pd.notna(row["OutputItem-1"]) else None,
                "OutputItemCount-1": int(row["OutputItemCount-1"]) if pd.notna(row["OutputItemCount-1"]) else 0,
                "OutputItem-2": row["OutputItem-2"] if pd.notna(row["OutputItem-2"]) else None,
                "OutputItemCount-2": int(row["OutputItemCount-2"]) if pd.notna(row["OutputItemCount-2"]) else 0,
                "OutputItem-3": row["OutputItem-3"] if pd.notna(row["OutputItem-3"]) else None,
                "OutputItemCount-3": int(row["OutputItemCount-3"]) if pd.notna(row["OutputItemCount-3"]) else 0,
                "OutputItem-4": row["OutputItem-4"] if pd.notna(row["OutputItem-4"]) else None,
                "OutputItemCount-4": int(row["OutputItemCount-4"]) if pd.notna(row["OutputItemCount-4"]) else 0,
                "CraftingTime": row["CraftingTime"]
            }

        # 결과를 JSON 파일로 저장
        with open(output_file, "w", encoding="utf-8") as json_file:
            json.dump(result, json_file, ensure_ascii=False, indent=2)

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
