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

            # Icon 필드 처리: '/Resources/' 제거하고 '.png' 확장자 제거
            icon = row["Icon"] if pd.notna(row["Icon"]) else ""
            if "/Resources/" in icon:
                icon = icon.replace("/Resources/", "").replace(".png", "")

            # ItemMesh 필드 처리: 앞에 '/'가 있으면 제거
            item_mesh = row["ItemMesh"] if pd.notna(row["ItemMesh"]) else ""
            if item_mesh.startswith("/"):
                item_mesh = item_mesh[1:]  # 첫 번째 문자 '/' 제거

            # DropMesh 필드 처리: 앞에 '/'가 있으면 제거
            drop_mesh = row["DropMesh"] if pd.notna(row["DropMesh"]) else ""
            if drop_mesh.startswith("/"):
                drop_mesh = drop_mesh[1:]  # 첫 번째 문자 '/' 제거

            # NaN 값 처리를 위해 pd.isna() 사용
            result["dic"][id] = {
                "Id": id,
                "Id_UShort": row["Id_UShort"],
                "ItemType": row["ItemType"] if pd.notna(row["ItemType"]) else None,  # NaN 처리
                "Name": row["Name"] if pd.notna(row["Name"]) else "",               # NaN 처리 (빈 문자열로)
                "Desc": row["Desc"] if pd.notna(row["Desc"]) else "",               # NaN 처리 (빈 문자열로)
                "MaxStack": row["MaxStack"] if pd.notna(row["MaxStack"]) else 0,    # NaN 처리 (숫자 기본값)
                "EnergyReserves": float(f"{row['EnergyReserves']:.1f}") if pd.notna(row["EnergyReserves"]) else 0.0,  # 소수점 1자리까지 강제 유지
                "Icon": icon,  # 수정된 Icon 경로
                "ItemMesh": item_mesh,  # 수정된 ItemMesh 경로
                "DropMesh": drop_mesh    # 수정된 DropMesh 경로
            }

        # 결과를 JSON 파일로 저장 (indent=2로 설정)
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
